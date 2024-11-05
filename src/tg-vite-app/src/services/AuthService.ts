import axios from 'axios'

import apiClient from './axiosConfig'

const BASE_URL = ''

export default class AuthService {
  async login(
    userId: number,
    walletAddress: any,
    username: string
  ): Promise<{ accessToken: string; refreshToken: string }> {
    try {
      const user = new UserDto(userId, username, walletAddress)
      const response = await apiClient.post(`${BASE_URL}/users/login`, user)
      localStorage.setItem('accessToken', response.data.accessToken)
      localStorage.setItem('refreshToken', response.data.refreshToken)
      return response.data
    } catch (error) {
      console.error(error)
      throw error
    }
  }

  async logout(): Promise<void> {
    try {
      await apiClient.post(`${BASE_URL}/users/logout`)
      localStorage.removeItem('accessToken')
      localStorage.removeItem('refreshToken')
    } catch (error) {
      console.error(error)
      throw error
    }
  }

  async refreshToken(): Promise<void> {
    try {
      const refreshToken = localStorage.getItem('refreshToken')
      const response = await apiClient.post(`${BASE_URL}/users/refresh`, { refreshToken })
      localStorage.setItem('accessToken', response.data.accessToken)
      localStorage.setItem('refreshToken', response.data.refreshToken)
    } catch (error) {
      console.error('Error refreshing token:', error)
      throw error
    }
  }

  async checkUserExists(userId: number): Promise<boolean> {
    try {
      const response = await apiClient.get(`${BASE_URL}/users/exists/${userId}`)
      return response.data.exists
    } catch (error) {
      console.error(error)
      throw error
    }
  }

  async getUserById(): Promise<UserDto> {
    try {
      const response = await apiClient.get(`${BASE_URL}/users`)
      return response.data as UserDto
    } catch (error) {
      console.error(error)
      throw error
    }
  }
}

export class UserDto {
  id: number
  username: string
  walletAddress: string

  constructor(id: number, username: string, walletAddress: string) {
    this.id = id
    this.username = username
    this.walletAddress = walletAddress
  }
}
