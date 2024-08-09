<template>
  <div v-if="jettons != undefined" class="wallet">
    <header>
      <h2>Welcome, {{ user?.username }}</h2>
      <div class="settings-wrapper">
        <div class="settings-icon" @click="toggleMenu">
          <img src="@/assets/settings-svgrepo-com.svg" alt="Settings" />
        </div>
        <div v-if="isMenuOpen" class="dropdown-menu">
          <button class="dropdown-item" @click="disconnect">Disconnect Wallet</button>
        </div>
      </div>
    </header>
    <div class="info">
      <p>Your balance is:</p>
      <h1>${{ balance }}</h1>
      <div class="address" @click="copyAddress(user?.rawWalletAddress)">
        {{ formatAddress(user?.rawWalletAddress) }}
      </div>
    </div>
    <div class="filters">
      <label class="checkbox-label">
        <input type="checkbox" v-model="hideZeroBalance" />
        <span>Hide zero balances</span>
      </label>
      <div class="sort-container">
        <select v-model="sortBy" class="sort-select">
          <option value="name">Sort by name</option>
          <option value="balance">Sort by balance</option>
        </select>
      </div>
    </div>
    <transition name="fade">
      <ul v-if="filteredJettons.length != 0" class="jettons-list">
        <li
          @click="navigateToJetton(jetton)"
          v-for="jetton in filteredJettons"
          :key="jetton.jetton.address"
          class="jetton-item"
        >
          <img :src="jetton.jetton.image" :alt="jetton.jetton.name" class="jetton-icon" />
          <div class="jetton-info">
            <span class="bold">{{ jetton.jetton.name }} </span>
            <div class="info-bottom">
              <span class="gray-small"> ${{ jetton.price.prices.usd.toFixed(2) }} </span>
              <span :class="getColorClass(jetton.price.diff24h.usd)">
                {{ jetton.price.diff24h.usd }}
              </span>
            </div>
          </div>
          <div class="jetton-balance">
            <span class="bold">{{ formatBalance(jetton) }}</span>
            <span class="gray-small"> $ {{ calculateJettonBalance(jetton) }} </span>
          </div>
        </li>
      </ul>
    </transition>
  </div>
  <div v-else>Loading...</div>
</template>

<script setup lang="ts">
import { useTonConnectStore } from '@/stores/store'
import { TonConnectUI } from '@tonconnect/ui'
import { computed, nextTick, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import TonConnectUiButton from '@/components/TonConnectUiButton.vue'
import AuthService, { UserDto } from '@/services/AuthService'
import type { Balance } from '@/services/types'
import TonService from '@/services/TonService'
import { useJettonStore } from '@/stores/jettonStore'

const authService = new AuthService()
const tonService = new TonService()

const route = useRoute()
const router = useRouter()
const userId = route.query.userId as string
const tonConnectStore = useTonConnectStore()
const user = ref<UserDto | null>(null)
const jettons = ref<Balance[]>([])
const hideZeroBalance = ref<boolean>(false)
const sortBy = ref<string>('name')
const balance = ref<number>(0)
const isMenuOpen = ref<boolean>(false)

const formatAddress = (address: string | undefined) => {
  if (!address) return ''
  const start = address.slice(0, 4)
  const end = address.slice(-4)
  return `${start}...${end}`
}

const animateBalance = (target: number) => {
  const duration = 500
  const startTime = performance.now()
  const startBalance = 0

  const step = (currentTime: number) => {
    const elapsedTime = currentTime - startTime
    const progress = Math.min(elapsedTime / duration, 1)
    balance.value = parseFloat((startBalance + (target - startBalance) * progress).toFixed(2))

    if (progress < 1) {
      requestAnimationFrame(step)
    }
  }

  requestAnimationFrame(step)
}

const copyAddress = (address: string | undefined) => {
  if (!address) return
  navigator.clipboard.writeText(address).catch((err) => {
    console.error('Failed to copy: ', err)
  })
}

const formatBalance = (jetton: Balance) => {
  return (parseFloat(jetton.balanceAmount) / Math.pow(10, jetton.jetton.decimals)).toFixed(2)
}

const calculateJettonBalance = (jetton: Balance) => {
  return (
    (parseFloat(jetton.balanceAmount) / Math.pow(10, jetton.jetton.decimals)) *
    jetton.price.prices.usd
  ).toFixed(2)
}

const getColorClass = (value: string) => {
  return value.startsWith('−') ? 'negative' : 'positive'
}

const calculateBalance = () => {
  return jettons.value
    .reduce((total, jetton) => {
      const jettonValue =
        (parseFloat(jetton.balanceAmount) / Math.pow(10, jetton.jetton.decimals)) *
        jetton.price.prices.usd
      return total + jettonValue
    }, 0)
    .toFixed(2)
}

const navigateToJetton = (jetton: Balance) => {
  const jettonStore = useJettonStore()
  jettonStore.setJetton(jetton)
  router.push({
    name: 'jettonDetail',
    query: {
      userId: userId
    }
  })
}

const filteredJettons = computed(() => {
  let filtered = jettons.value.slice()

  if (hideZeroBalance.value) {
    filtered = filtered.filter(
      (jetton) =>
        (parseFloat(jetton.balanceAmount) / Math.pow(10, jetton.jetton.decimals)) *
          jetton.price.prices.usd >
        0.01
    )
  }

  if (sortBy.value === 'name') {
    filtered.sort((a, b) => a.jetton.name.localeCompare(b.jetton.name))
  } else if (sortBy.value === 'balance') {
    filtered.sort((a, b) => {
      const balanceA =
        (parseFloat(a.balanceAmount) / Math.pow(10, a.jetton.decimals)) * a.price.prices.usd
      const balanceB =
        (parseFloat(b.balanceAmount) / Math.pow(10, b.jetton.decimals)) * b.price.prices.usd
      return balanceB - balanceA
    })
  }

  const toncoinIndex = filtered.findIndex((jetton) => jetton.jetton.name === 'Toncoin')
  if (toncoinIndex > -1) {
    const [toncoinJetton] = filtered.splice(toncoinIndex, 1)
    filtered.unshift(toncoinJetton)
  }

  return filtered
})

const toggleMenu = () => {
  isMenuOpen.value = !isMenuOpen.value
}

const disconnect = async () => {
  if (userId) {
    await tonConnectStore.disconnect(parseFloat(userId))
    router.push({ name: 'welcome' })
  }
}

onMounted(async () => {
  user.value = await authService.getUserById(Number(userId))
  if (user.value) {
    jettons.value = await tonService.getJettons(Number(userId))
    const targetBalance = parseFloat(calculateBalance())
    animateBalance(targetBalance)
  }
})
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

h1 {
  font-weight: 550;
  font-size: 2.2rem;
  position: relative;
}

.info {
  padding: 12px;
  justify-content: space-between;
  text-align: center;
  align-items: center;
}

.wallet {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 100%;
  color: #fff;
  font-size: 16px;
  position: relative;
}

header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 12px 0px 12px;
}

.settings-wrapper {
  position: relative;
}

.settings-icon {
  cursor: pointer;
  display: flex;
  align-items: center;
}

.settings-icon img {
  width: 24px;
  height: 24px;
}

.dropdown-menu {
  position: absolute;
  top: 40px;
  right: 0;
  background-color: #d42230;
  border-radius: 8px;
  box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.3);
  z-index: 10;
  min-width: 150px;
}

.dropdown-item {
  padding: 10px 10px;
  width: 100%;
  border: none;
  background: none;
  color: #d1d4dc;
  text-align: center;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.info {
  padding: 12px;
  justify-content: space-between;
  text-align: center;
  align-items: center;
}

.filters {
  display: flex;
  align-items: center;
  padding: 10px;
  box-shadow: 8px 4px 8px 8px rgba(0, 0, 0, 0.1);
}

.checkbox-label {
  display: flex;
  align-items: center;
  margin-right: 20px;
  color: #d1d4dc;
  font-size: 14px;
}

.checkbox-label input[type='checkbox'] {
  margin-right: 5px;
  width: 18px;
  height: 18px;
  appearance: none;
  border: 1px solid #2a3240;
  background-color: #1b2330;
  border-radius: 3px;
  position: relative;
  cursor: pointer;
  outline: none;
}

.checkbox-label input[type='checkbox']::after {
  content: '';
  position: absolute;
  top: 2px;
  left: 2px;
  width: 10px;
  height: 10px;
  border-radius: 2px;
  background-color: transparent;
  transition: background-color 0.3s ease;
}

.checkbox-label input[type='checkbox']:checked::after {
  background-color: #00bfff;
}

.sort-container {
  position: relative;
  display: flex;
  align-items: center;
  flex-grow: 1;
}

.sort-select {
  padding: 8px 12px;
  border-radius: 5px;
  border: 1px solid #343a40;
  background-color: #2a3240;
  color: #d1d4dc;
  width: 100%;
  appearance: none;
  font-size: 14px;
  outline: none;
}

.sort-select option {
  background-color: #2a3240;
  color: #d1d4dc;
  border-radius: 5px;
}

.sort-container::after {
  content: '▼';
  position: absolute;
  right: 10px;
  pointer-events: none;
  color: #d1d4dc;
  font-size: 10px;
}

.jettons-list {
  list-style-type: none;
  padding: 0.5rem 0.5rem;
}

.jetton-item {
  display: flex;
  align-items: center;
  margin-bottom: 10px;
  background-color: #1d2633;
  padding: 5px 10px;
  border-radius: 8px;
}

.jetton-icon {
  width: 40px;
  height: 40px;
  margin-right: 10px;
  border-radius: 50%;
}

.jetton-info {
  display: flex;
  flex-direction: column;
  flex-grow: 1;
}

.jetton-balance {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}

.address {
  cursor: default;
  transition: color 0.2s;
}

.address:active {
  color: rgb(63, 62, 155);
}
</style>
