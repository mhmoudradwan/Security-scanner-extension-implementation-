import apiClient from './axios.config';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

export const authApi = {
  // POST /api/auth/register
  register: async (userData) => {
    return apiClient.post('/auth/register', {
      email: userData.email,
      username: userData.username,
      firstName: userData.firstName,
      lastName: userData.lastName,
      password: userData.password,
      phoneNumber: userData.phoneNumber || null,
      gender: userData.gender || null,
      dateOfBirth: userData.dateOfBirth || null,
      country: userData.country || null,
      bio: userData.bio || null
    }); // { success: true, data: token }
  },

  // POST /api/auth/login
  login: async (credentials) => {
    return apiClient.post('/auth/login', {
      email: credentials.email,
      password: credentials.password
    }); // { success: true, data: token }
  },

  // POST /api/auth/validate-token
  validateToken: async (token) => {
    return apiClient.post('/auth/validate-token', token);
  },

  // GET /api/users/profile
  getProfile: async () => {
    return apiClient.get('/users/profile');
  },

  // PUT /api/users/profile
  updateProfile: async (data) => {
    return apiClient.put('/users/profile', data);
  },

  // DELETE /api/users/profile
  deleteAccount: async () => {
    return apiClient.delete('/users/profile');
  },

  // PUT /api/users/change-password
  changePassword: async (newPassword) => {
    return apiClient.put('/users/change-password', { newPassword });
  },

  // POST /api/password-reset/request
  requestPasswordReset: async (email) => {
    return apiClient.post('/password-reset/request', { email });
  },

  // POST /api/password-reset/reset
  resetPassword: async (token, newPassword) => {
    return apiClient.post('/password-reset/reset', { token, newPassword });
  },

  // GET /api/password-reset/validate/{token}
  validateResetToken: async (token) => {
    return apiClient.get(`/password-reset/validate/${encodeURIComponent(token)}`);
  },

  // POST /api/users/profile-picture (multipart/form-data)
  uploadProfilePicture: async (file) => {
    const formData = new FormData();
    formData.append('file', file);
    return apiClient.post('/users/profile-picture', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
  },

  // DELETE /api/users/profile-picture
  deleteProfilePicture: async () => {
    return apiClient.delete('/users/profile-picture');
  }
};

