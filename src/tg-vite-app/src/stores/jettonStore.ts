import type { Balance } from '@/services/types'
import { defineStore } from 'pinia'

export const useJettonStore = defineStore('jetton', {
  state: () => ({
    selectedJetton: null as Balance | null
  }),
  actions: {
    setJetton(jetton: Balance) {
      this.selectedJetton = jetton
    }
  }
})
