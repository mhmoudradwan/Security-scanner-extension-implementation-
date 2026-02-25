// Baseera Security Scanner - Content Script
// Passive observer - does not modify the page

(function() {
  'use strict';
  
  // Listen for messages from the popup
  chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    if (message.type === 'PING') {
      sendResponse({ status: 'ready', url: window.location.href });
    }
  });

  // Listen for custom events dispatched by the web app for auth state changes
  window.addEventListener('baseeraAuthLogin', (event) => {
    const { token, user } = event.detail || {};
    if (token) {
      chrome.runtime.sendMessage({
        type: 'LOGIN_SUCCESS',
        payload: { token, user }
      });
    }
  });

  window.addEventListener('baseeraAuthLogout', () => {
    chrome.runtime.sendMessage({ type: 'LOGOUT' });
  });
})();
