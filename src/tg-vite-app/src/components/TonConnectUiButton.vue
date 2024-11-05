<template>
  <div class="wallet-buttons">
    <button
      class="wallet-button"
      :class="{ connected: isConnected }"
      @click="isConnected ? copyAddress() : openConnectModal()"
    >
      {{ isConnected ? 'Copy Address' : 'Connect Wallet' }}
    </button>
    <button v-if="isConnected" class="wallet-button disconnect-button" @click="disconnect">
      Disconnect
    </button>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useTonConnectStore } from '@/stores/store'
import { useRouter } from 'vue-router'
import AuthService from '@/services/AuthService'

const tonConnectStore = useTonConnectStore()
const isModalOpen = ref(false)
let unsubscribe: (() => void) | null = null
const userId = ref<number | null>(null)
const authService = new AuthService()
const router = useRouter()

const isConnected = computed(() => tonConnectStore.tonConnectUI?.connected)

const openConnectModal = async () => {
  tonConnectStore.openModal()
  isModalOpen.value = true
}

const copyAddress = async () => {
  const address = tonConnectStore.tonConnectUI?.wallet?.account?.address
  if (address) {
    navigator.clipboard.writeText(address).then(() => {
      alert('Address copied to clipboard')
    })
  }
}

const disconnect = async () => {
  if (userId.value) {
    await tonConnectStore.disconnect()
    router.push({ name: 'welcome' })
  }
}

onMounted(async () => {
  const tonConnectUI = tonConnectStore.tonConnectUI
  if (!tonConnectUI) await tonConnectStore.initializeTonConnectUI()

  await loadTelegramSDK()
  if (window.Telegram.WebApp) {
    const tg = window.Telegram.WebApp
    const user = tg.initDataUnsafe.user
    userId.value = user.id
    if (tonConnectStore.tonConnectUI) {
      tonConnectStore.tonConnectUI.onStatusChange(async (walletAndwalletInfo) => {
        try {
          await authService.login(
            user.id,
            tonConnectStore.tonConnectUI?.account?.address,
            user.username
          )
          router.push({ name: 'home', query: { userId: user.id } })
        } catch (error) {
          console.error(error)
        }
      })
    }
  }
})

async function loadTelegramSDK(): Promise<void> {
  return new Promise<void>((resolve, reject) => {
    const script = document.createElement('script')
    script.src = 'https://telegram.org/js/telegram-web-app.js'
    script.async = true
    script.onload = () => resolve()
    script.onerror = () => reject(new Error('Failed to load Telegram SDK'))
    document.head.appendChild(script)
  })
}
</script>

<style scoped>
.wallet-buttons {
  display: flex;
  gap: 10px;
}

.wallet-button {
  background-color: #3b1cee;
  color: white;
  border: none;
  border-radius: 8px;
  padding: 15px 100px;
  font-size: 20px;

  cursor: pointer;
  transition: background-color 0.3s;
}

.wallet-button:hover {
  background-color: #2224c2;
}

.wallet-button.connected {
  background-color: #28a745;
}

.disconnect-button {
  background-color: #dc3545;
}

.disconnect-button:hover {
  background-color: #c82333;
}
</style>
