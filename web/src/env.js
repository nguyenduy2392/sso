(function (window) {
  window.__env = window.__env || {};
  var host = window.location.hostname.toLowerCase();
  var isLocal = host === 'localhost' || host === '127.0.0.1';
  window.__env.apiUrl = isLocal ? 'http://localhost:5265' : '';
})(this);
