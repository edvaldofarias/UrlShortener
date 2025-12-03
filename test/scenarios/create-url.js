import http from 'k6/http';
import { check, sleep } from 'k6';

const BASE_URL = 'http://localhost:5001';

export default function() {
  const url = BASE_URL + '/api/shorten';
  const payload = JSON.stringify("https://site" + Math.floor(Math.random() * 200000) + ".com");
  const params = { headers: { 'Content-Type': 'application/json' } };

  let res = http.post(url, payload, params);

  check(res, { 'status é 201': (r) => r.status === 201 });

  sleep(1);
}