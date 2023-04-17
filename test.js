import http from 'k6/http';
import { sleep } from 'k6';
export let options = {
    stages: [
      { duration: '1m', target: 100 },
      { duration: '3m', target: 100 },
      { duration: '1m', target: 200 },
      { duration: '3m', target: 200 },
      { duration: '1m', target: 300 },
      { duration: '3m', target: 300 },
      { duration: '3m', target: 0 },
    ],
  };
  
  
  
  
export default function () {
  http.get('https://tweet-app-fauji-performance-test-aijk54j36a-et.a.run.app/'); // Ganti dengan cloudrun URL
  sleep(1);
}
