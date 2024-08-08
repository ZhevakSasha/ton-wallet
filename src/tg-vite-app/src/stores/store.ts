import { defineStore } from 'pinia'
import { TonConnectUI } from '@tonconnect/ui'
import { nextTick } from 'vue'

interface TonConnectState {
  tonConnectUI: TonConnectUI | null
}

export const useTonConnectStore = defineStore({
  id: 'tonConnect',
  state: (): TonConnectState => ({
    tonConnectUI: null as TonConnectUI | null
  }),
  actions: {
    async initializeTonConnectUI() {
      if (!this.tonConnectUI) {
        this.tonConnectUI = new TonConnectUI({
          manifestUrl: 'https://warm-hopefully-donkey.ngrok-free.app/tonconnect-manifest.json',
          buttonRootId: 'connect'
        })
      }
    }
  }
})
