import { defineStore } from 'pinia'
import { TonConnectUI } from '@tonconnect/ui'
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import AuthService from '@/services/AuthService'

interface TonConnectState {
  tonConnectUI: TonConnectUI | null
}

export const useTonConnectStore = defineStore('tonConnect', {
  state: (): TonConnectState => ({
    tonConnectUI: null as TonConnectUI | null
  }),
  actions: {
    async initializeTonConnectUI() {
      if (!this.tonConnectUI) {
        this.tonConnectUI = new TonConnectUI({
          manifestUrl: 'https://warm-hopefully-donkey.ngrok-free.app/tonconnect-manifest.json'
        })
      }

      this.tonConnectUI.onStatusChange((wallet) => {})
    },
    async openModal() {
      await this.tonConnectUI?.openModal()
    },
    async disconnect(userId: number) {
      const authService = new AuthService()
      await this.tonConnectUI?.disconnect()
      await authService.logout(userId)
    }
  }
})
