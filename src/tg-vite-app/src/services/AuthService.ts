import axios from 'axios'

const BASE_URL = 'https://localhost:7206/api'

export default class AuthService {
  async login(userId: number, walletAddress: any, username: string): Promise<void> {
    try {
      const user = new UserDto(userId, username, walletAddress)
      await axios.post(`${BASE_URL}/users/login`, user)
    } catch (error) {
      console.error(error)
      throw error
    }
  }

  async checkUserExists(userId: number): Promise<boolean> {
    try {
      const response = await axios.get(`${BASE_URL}/users/exists?id=${userId}`)
      return response.data
    } catch (error) {
      console.error(error)
      throw error
    }
  }

  async getUserById(userId: number): Promise<UserDto> {
    try {
      const response = await axios.get(`${BASE_URL}/users?id=${userId}`)
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
  rawWalletAddress: string

  constructor(id: number, username: string, rawWalletAddress: string) {
    this.id = id
    this.username = username
    this.rawWalletAddress = rawWalletAddress
  }
}
