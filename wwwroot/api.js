export class ApiError extends Error {
  constructor(message, status) { super(message); this.name = 'ApiError'; this.status = status; }
}
export function createApi({ base = '', getToken = () => '', onUnauthorized = () => {} } = {}) {
  return async function request(path, { method = 'GET', body, auth = false } = {}) {
    const headers = { Accept: 'application/json' };
    if (body !== undefined) headers['Content-Type'] = 'application/json';
    if (auth && getToken()) headers.Authorization = `Bearer ${getToken()}`;
    let response;
    try {
      response = await fetch(`${base.replace(/\/$/, '')}${path}`, {
        method, headers, body: body === undefined ? undefined : JSON.stringify(body),
        signal: AbortSignal.timeout(15000)
      });
    } catch { throw new ApiError('Не удалось связаться с сервером. Проверь, что приложение ASP.NET Core запущено, и попробуй ещё раз.', 0); }
    const raw = await response.text();
    let data = raw;
    try { data = raw ? JSON.parse(raw) : null; } catch { /* Some endpoints return plain text. */ }
    if (!response.ok) {
      if (response.status === 401 && auth) onUnauthorized();
      const message = data?.errors ? Object.values(data.errors).flat().join(' ') :
        response.status === 401 ? 'Сессия закончилась. Войди ещё раз.' :
        response.status === 403 ? 'Недостаточно прав для этого действия.' :
        response.status >= 500 ? 'Ошибка сервера. Попробуй позже.' :
        typeof data === 'string' && !data.includes('<') && data.length < 1000 ? data :
        data?.detail || data?.title || `Не удалось выполнить запрос (${response.status}).`;
      throw new ApiError(message, response.status);
    }
    return data;
  };
}
export function readClaims(token) {
  try {
    const part = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    const bytes = Uint8Array.from(atob(part), c => c.charCodeAt(0));
    return JSON.parse(new TextDecoder().decode(bytes));
  } catch { return {}; }
}
