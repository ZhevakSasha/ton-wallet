<template>
  <div class="welcome-container">
    <div class="welcome-message">
      <h1>Welcome</h1>
    </div>
    <TonConnectUiButton></TonConnectUiButton>
  </div>
</template>

<script setup lang="ts">
import WelcomeItem from './WelcomeItem.vue'
import DocumentationIcon from './icons/IconDocumentation.vue'
import ToolingIcon from './icons/IconTooling.vue'
import EcosystemIcon from './icons/IconEcosystem.vue'
import CommunityIcon from './icons/IconCommunity.vue'
import SupportIcon from './icons/IconSupport.vue'
import { CHAIN } from '@tonconnect/sdk'
import { TonConnectUI } from '@tonconnect/ui'
import { computed, onMounted, ref } from 'vue'
import AuthService from '@/services/AuthService'
import { useRouter } from 'vue-router'
import { useTonConnectStore } from '@/stores/store'
import TonConnectUiButton from '@/components/TonConnectUiButton.vue'

let unsubscribe: (() => void) | null = null
const userId = ref<number | null>(null)
const authService = new AuthService()
const router = useRouter()
const tonConnectStore = useTonConnectStore()

onMounted(async () => {
  tonConnectStore.initializeTonConnectUI
  let tonConnectUI = tonConnectStore.tonConnectUI
  await loadTelegramSDK()
  if (window.Telegram.WebApp) {
    const tg = window.Telegram.WebApp
    const user = tg.initDataUnsafe.user
    if (await authService.checkUserExists(user.id)) {
      router.push({ name: 'home', query: { userId: user.id } })
      return
    }
    if (tonConnectUI) {
      unsubscribe = tonConnectUI.onStatusChange(async (walletAndwalletInfo) => {
        try {
          await authService.login(user.id, tonConnectUI?.account?.address, user.username)
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
.welcome-container {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 100%;
  color: white;
}

.welcome-message {
  text-align: center;
  margin-bottom: 20px;
}

button {
  background-color: #333;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 5px;
  cursor: pointer;
}

button:hover {
  background-color: #444;
}
</style>
