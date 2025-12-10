import http from 'k6/http';
import { check, sleep } from 'k6';

const BASE_URL = 'http://localhost';

// setup() roda antes dos VUs e retorna dados para todas as VUs
export function setup() {
  const iterations = 1000; // ajuste conforme necessário
  const seed = [];
  const url = BASE_URL + '/api/shorten';
  const params = { headers: { 'Content-Type': 'application/json' } };

  for (let i = 0; i < iterations; i++) {
    const payload = JSON.stringify("https://test"+ Math.floor(Math.random() * (iterations * 10)) + "seed" + Math.floor(Math.random() * (iterations * 10)) + ".com");
    let res = http.post(url, payload, params);
    if (res.status === 201 && res.body) {
      seed.push(res.body.replace(/"/g, ''));
    }
  }
  return { shortUrls: seed };
}



export default function (data) {
  const available = (data && data.shortUrls) ? data.shortUrls : [];
  if (available.length === 0) {
    console.log('Nenhum URL curto disponível para leitura.');
    return;
  }

  let fullUrl = available[Math.floor(Math.random() * available.length)];
  const params = { redirects: 0 }; // não seguir redirects
  let res = http.get(fullUrl, params);

  check(res, { 'status é 404 ou redirect': (r) => r.status === 404 || [301,302,307,308].includes(r.status) });

  sleep(0.2);
}