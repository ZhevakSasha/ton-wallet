import axios from 'axios'
import AuthService from './AuthService'
import { eventBus } from './eventBus'

const BASE_URL = '/api'

const apiClient = axios.create({
  baseURL: BASE_URL
})

let isRefreshing = false
let failedQueue: any[] = []

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach(prom => {
    if (error) {
      prom.reject(error)
    } else {
      prom.resolve(token)
    }
  })
  
  failedQueue = []
}

apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

apiClient.interceptors.response.use(
  (response) => {
    return response
  },
  async (error) => {
    const originalRequest = error.config
    
    if (error.response.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject })
        }).then(token => {
          originalRequest.headers.Authorization = `Bearer ${token}`
          return apiClient(originalRequest)
        }).catch(err => {
          return Promise.reject(err)
        })
      }

      originalRequest._retry = true
      isRefreshing = true

      try {
        const authService = new AuthService()
        await authService.refreshToken()
        const newToken = localStorage.getItem('accessToken')
        isRefreshing = false
        originalRequest.headers.Authorization = `Bearer ${newToken}`
        processQueue(null, newToken)
        return apiClient(originalRequest)
      } catch (refreshError) {
        console.error('Error refreshing token:', refreshError)
        isRefreshing = false
        processQueue(refreshError, null)
        
        console.log('Emitting authError event')
        window.Telegram.WebApp.showAlert('Emitting authError event')
        
        eventBus.emit('authError')
        return Promise.reject(new Error('AUTH_ERROR'))
      }
    }
    return Promise.reject(error)
  }
)

export default apiClient
