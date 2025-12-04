import { group, sleep } from 'k6';
import CreateUrl from './scenarios/create-url.js';
import ReadUrl, { setup as readUrlSetup } from './scenarios/read-url.js';

export const test = {
    createUrl: CreateUrl,
    readUrl: ReadUrl,
};

export function setup() {
    return readUrlSetup();
}

export let options = {
    thresholds: {
        http_req_duration: ['p(95)<200'], // 95% das requisições devem ser menores que 200ms
        http_req_failed: ['rate<0.01'], // menos de 1% de falhas

    },
    scenarios: {
        create_url: {
            executor: 'constant-vus',
            vus: 1000,
            duration: '5m',
            exec: 'createUrl',
        },
        read_url: {
            executor: 'per-vu-iterations',
            vus: 10000,
            iterations: 30,
            startTime: '15s',
            maxDuration: '10m',
            exec: 'readUrl'
        },
    },
};

export function createUrl() {
    group('Teste de Criação de URL Curta', function () {
        CreateUrl();
    });
    sleep(1);
}

export function readUrl(data) {
    group('Teste de Leitura de URL Curta', function () {
        ReadUrl(data);
    });
    sleep(1);
}