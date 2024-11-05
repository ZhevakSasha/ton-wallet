import axios from 'axios'
import type { Balance, Point, TransactionHistory } from './types'

import apiClient from './axiosConfig';

const BASE_URL = '/ton';

export default class TonService {
  async getJettons(): Promise<Balance[]> {
    try {
      const response = await apiClient.get<Balance[]>(`${BASE_URL}/jettons`);
      return response.data;
    } catch (error) {
      console.error('Error fetching jettons:', error);
      throw error;
    }
  }

  async getTonHistory(): Promise<TransactionHistory[]> {
    try {
      const response = await apiClient.get<TransactionHistory[]>(`${BASE_URL}/tonHistory`);
      return response.data;
    } catch (error) {
      console.error('Error fetching TON history:', error);
      throw error;
    }
  }

  async getJettonHistory(jettonAddress: string): Promise<TransactionHistory[]> {
    try {
      const response = await apiClient.get<TransactionHistory[]>(`${BASE_URL}/jettonHistory?jettonAddress=${jettonAddress}`);
      return response.data;
    } catch (error) {
      console.error('Error fetching jetton history:', error);
      throw error;
    }
  }

  async getJettonChart(jettonAddress: string, startDate: number): Promise<Point[]> {
    try {
      const response = await apiClient.get<Point[]>(`${BASE_URL}/jettonChart?jettonAddress=${jettonAddress}&startDate=${startDate}`);
      return response.data;
    } catch (error) {
      console.error('Error fetching jetton chart:', error);
      throw error;
    }
  }
}
