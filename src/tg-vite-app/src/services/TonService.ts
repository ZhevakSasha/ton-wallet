import axios from 'axios'
import type { Balance, Point, TransactionHistory } from './types'

const BASE_URL = 'https://localhost:7206/api/users'

export default class TonService {
  async getJettons(userId: number): Promise<Balance[]> {
    try {
      const response = await axios.get<Balance[]>(`${BASE_URL}/jettons?userId=${userId}`)
      return response.data
    } catch (error) {
      console.error('Error fetching jettons:', error)
      throw error
    }
  }

  async getTonHistory(userId: string): Promise<TransactionHistory[]> {
    try {
      const response = await axios.get<TransactionHistory[]>(
        `${BASE_URL}/tonHistory?userId=${userId}`
      )
      return response.data
    } catch (error) {
      console.error('Error fetching jettons:', error)
      throw error
    }
  }

  async getJettonHistory(userId: string, jettonAddress: string): Promise<TransactionHistory[]> {
    try {
      const response = await axios.get<TransactionHistory[]>(
        `${BASE_URL}/jettonHistory?userId=${userId}&jettonAddress=${jettonAddress}`
      )
      return response.data
    } catch (error) {
      console.error('Error fetching jettons:', error)
      throw error
    }
  }

  async getJettonChart(jettonAddress: string, startDate: number): Promise<Point[]> {
    try {
      const response = await axios.get<Point[]>(
        `${BASE_URL}/jettonChart?jettonAddress=${jettonAddress}&StartDate=${startDate}`
      )
      return response.data
    } catch (error) {
      console.error('Error fetching jettons:', error)
      throw error
    }
  }
}
