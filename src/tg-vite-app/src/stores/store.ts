import { defineStore } from 'pinia'
import { TonConnectUI, type SendTransactionRequest } from '@tonconnect/ui'
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import AuthService from '@/services/AuthService'

interface TonConnectState {
  tonConnectUI: TonConnectUI | null
}

function toNano(value: string): string {
  const [wholePart, decimalPart = ''] = value.split('.')

  const wholePartNano = BigInt(wholePart) * BigInt(1e9)

  const decimalPartNano = BigInt(decimalPart.padEnd(9, '0').slice(0, 9))

  const nanoAmount = wholePartNano + decimalPartNano

  return nanoAmount.toString()
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
      await this.tonConnectUI?.disconnect()
    },
    async disconnect() {
      await this.tonConnectUI?.disconnect()
    },
    async sendTransaction(toAddress: string, amount: string) {
      try {
        const senderAddress = this.tonConnectUI?.wallet?.account.address
        if (!senderAddress) {
          throw new Error('Wallet is not connected')
        }

        const transaction: SendTransactionRequest = {
          validUntil: Math.floor(Date.now() / 1000) + 300,
          messages: [
            {
              address: toAddress,
              amount: toNano(amount),
              payload: ''
            }
          ]
        }

        const result = await this.tonConnectUI?.sendTransaction(transaction)

        return result
      } catch (error) {
        console.error('Failed to send transaction:', error)
        throw error
      }
    }
  }
})
