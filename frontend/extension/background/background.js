// Baseera Security Scanner - Background Service Worker

chrome.runtime.onInstalled.addListener(() => {
  console.log('Baseera Security Scanner installed');
});

// Listen for messages from popup, content scripts, or web app (externally_connectable)
chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  if (message.type === 'LOGIN_SUCCESS') {
    const { token, user } = message.payload || {};
    chrome.storage.local.set({
      authToken: token,
      userName: user?.username || user?.userName || ''
    }, () => {
      chrome.action.setBadgeText({ text: '✓' });
      chrome.action.setBadgeBackgroundColor({ color: '#22c55e' });
      sendResponse({ success: true });
    });
    return true;
  }

  if (message.type === 'LOGOUT') {
    chrome.storage.local.remove(['authToken', 'userName'], () => {
      chrome.action.setBadgeText({ text: '' });
      sendResponse({ success: true });
    });
    return true;
  }

  if (message.type === 'SAVE_AUTH') {
    chrome.storage.local.set({
      authToken: message.token,
      userName: message.userName
    }, () => {
      sendResponse({ success: true });
    });
    return true;
  }

  if (message.type === 'CLEAR_AUTH') {
    chrome.storage.local.remove(['authToken', 'userName'], () => {
      chrome.action.setBadgeText({ text: '' });
      sendResponse({ success: true });
    });
    return true;
  }

  if (message.type === 'GET_AUTH') {
    chrome.storage.local.get(['authToken', 'userName'], (result) => {
      sendResponse({ token: result.authToken, userName: result.userName });
    });
    return true;
  }
});

// Handle messages from externally_connectable web app
chrome.runtime.onMessageExternal.addListener((message, sender, sendResponse) => {
  if (message.type === 'LOGIN_SUCCESS') {
    const { token, user } = message.payload || {};
    chrome.storage.local.set({
      authToken: token,
      userName: user?.username || user?.userName || ''
    }, () => {
      chrome.action.setBadgeText({ text: '✓' });
      chrome.action.setBadgeBackgroundColor({ color: '#22c55e' });
      sendResponse({ success: true });
    });
    return true;
  }

  if (message.type === 'LOGOUT') {
    chrome.storage.local.remove(['authToken', 'userName'], () => {
      chrome.action.setBadgeText({ text: '' });
      sendResponse({ success: true });
    });
    return true;
  }
});
