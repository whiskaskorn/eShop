import test from 'node:test';
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
const source = await readFile(new URL('../wwwroot/api.js', import.meta.url), 'utf8');
const { createApi, readClaims } = await import(`data:text/javascript;base64,${Buffer.from(source).toString('base64')}`);
test('API contracts', async t => {
 const original = globalThis.fetch;
 try {
  await t.test('login sends username, email and password; reads JSON token', async () => {
   globalThis.fetch = async (url, options) => {assert.equal(url,'/users/login');assert.deepEqual(JSON.parse(options.body),{username:'tester',email:'a@b.com',password:'secret'});assert.equal(options.headers.Authorization,undefined);return new Response(JSON.stringify('jwt-token'));};
   assert.equal(await createApi()('/users/login',{method:'POST',body:{username:'tester',email:'a@b.com',password:'secret'}}),'jwt-token');
  });
  await t.test('authorized product update preserves API spelling and boolean', async () => {
   globalThis.fetch = async (url, options) => {assert.equal(url,'/products/4');assert.equal(options.method,'PUT');assert.equal(options.headers.Authorization,'Bearer token');assert.deepEqual(JSON.parse(options.body),{name:'Lamp',decription:'A lovely desk lamp',price:12.5,isActive:false});return new Response(null,{status:200});};
   assert.equal(await createApi({getToken:()=> 'token'})('/products/4',{method:'PUT',auth:true,body:{name:'Lamp',decription:'A lovely desk lamp',price:12.5,isActive:false}}),null);
  });
  await t.test('204 deletion response does not require JSON', async()=>{globalThis.fetch=async()=>new Response(null,{status:204});assert.equal(await createApi()('/products/4',{method:'DELETE'}),null);});
  await t.test('FluentValidation errors are readable',async()=>{globalThis.fetch=async()=>new Response(JSON.stringify({errors:{Name:['Name is required'],Price:['Must be positive']}}),{status:400});await assert.rejects(createApi()('/products/'),/Name is required Must be positive/);});
  await t.test('401 clears authenticated session',async()=>{let cleared=false;globalThis.fetch=async()=>new Response(null,{status:401});await assert.rejects(createApi({onUnauthorized:()=>cleared=true})('/users/me',{auth:true}),/Сессия/);assert.equal(cleared,true);});
  await t.test('network errors give actionable message',async()=>{globalThis.fetch=async()=>{throw new TypeError('fetch failed');};await assert.rejects(createApi()('/products/'),/ASP.NET Core/);});
  await t.test('JWT claims support Unicode; invalid token is safe',()=>{const claims={Username:'Пример',Id:'1'};const token=`x.${Buffer.from(JSON.stringify(claims)).toString('base64url')}.x`;assert.deepEqual(readClaims(token),claims);assert.deepEqual(readClaims('bad'),{});});
 } finally {globalThis.fetch=original;}
});
