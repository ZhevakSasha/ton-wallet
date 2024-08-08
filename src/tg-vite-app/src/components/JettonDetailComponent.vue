<template>
  <div v-if="jetton" class="wallet-container">
    <div class="top-bar">
      <button @click="goBack" class="back-button">🢀</button>
      <h1 class="title">{{ jetton?.jetton.name }}</h1>
    </div>

    <div class="header">
      <div class="balance-info">
        <p class="balance-info-top">
          {{ formatBalance(jetton?.balanceAmount, jetton.jetton.decimals) }}
          {{ jetton?.jetton.symbol }}
        </p>
        <p class="balance-info-bottom">${{ calculateJettonBalance(jetton) }}</p>
      </div>
      <img :src="jetton?.jetton.image" :alt="jetton?.jetton.name" class="jetton-icon" />
    </div>

    <div v-if="selectedPoint !== null" class="price-info">
      <h3>${{ selectedPoint.value.toFixed(4) }}</h3>
      <p :class="getColorClass(percentageChange.toFixed(2))">{{ percentageChange.toFixed(2) }} %</p>
      <span v-if="!isTouched" class="gray-small"> Price </span>
      <span v-else class="gray-small"> {{ getDate(selectedPoint.utime.toString()) }} </span>
    </div>

    <div
      class="chart"
      @mousedown="handleStart"
      @mouseup="handleEnd"
      @mousemove="handleMove"
      @touchstart="handleStart"
      @touchend="handleEnd"
      @touchmove="handleMove"
    >
      <JettonChart
        :points="points"
        :selected-period="selectedPeriod"
        @point-selected="handlePointSelected"
      />
    </div>

    <div class="timeframe-controls">
      <button :class="{ active: selectedPeriod === '1h' }" @click="setPeriod('1h')">1H</button>
      <button :class="{ active: selectedPeriod === '1d' }" @click="setPeriod('1d')">1D</button>
      <button :class="{ active: selectedPeriod === '7d' }" @click="setPeriod('7d')">7D</button>
      <button :class="{ active: selectedPeriod === '1m' }" @click="setPeriod('1m')">1M</button>
      <button :class="{ active: selectedPeriod === '6m' }" @click="setPeriod('6m')">6M</button>
      <button :class="{ active: selectedPeriod === '1y' }" @click="setPeriod('1y')">1Y</button>
    </div>
    <transition name="fade">
      <div class="history" v-if="jettonHistory.length != 0">
        <ul class="history-list">
          <li v-for="history in jettonHistory" :key="history.utime" class="history-item">
            <div class="left-info">
              <span class="bold"> {{ history.transactionType }} </span>
            </div>
            <div class="right-info">
              <span class="bold">
                {{ formatBalance(history.value.toString(), jetton.jetton.decimals) }}
                {{ jetton.jetton.symbol }}</span
              >
              <span class="gray-small"> {{ getDate(history.utime) }} </span>
            </div>
          </li>
        </ul>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
import { useTonConnectStore } from '@/stores/store'
import { TonConnectUI } from '@tonconnect/ui'
import { computed, nextTick, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import TonConnectUiButton from '@/components/TonConnectUiButton.vue'
import AuthService, { UserDto } from '@/services/AuthService'
import type { Balance, Point, TransactionHistory } from '@/services/types'
import TonService from '@/services/TonService'
import { useJettonStore } from '@/stores/jettonStore'
import JettonChart from '@/components/JettonChart.vue'

const authService = new AuthService()
const tonService = new TonService()
const tonConnectStore = useTonConnectStore()

const route = useRoute()
const router = useRouter()
const userId = route.query.userId as string
const jettonStore = useJettonStore()
const jettonHistory = ref<TransactionHistory[]>([])
const points = ref<Point[]>([])
const selectedPeriod = ref('1d')
const percentageChange = ref(0)
const selectedPoint = ref<Point | null>(null)

const isTouched = ref(false)

const handleStart = () => {
  isTouched.value = true
  console.log(isTouched.value)
}

const handleEnd = () => {
  isTouched.value = false
}

const handlePointSelected = (value: Point | null) => {
  selectedPoint.value = value
  calculatePercentageChange(points.value)
}

const goBack = () => {
  router.back()
}

const setPeriod = async (period: string) => {
  selectedPeriod.value = period
  const now = new Date().getTime()

  let timeDiff
  switch (period) {
    case '1h':
      timeDiff = 1 * 60 * 60 * 1000
      break
    case '1d':
      timeDiff = 24 * 60 * 60 * 1000
      break
    case '7d':
      timeDiff = 7 * 24 * 60 * 60 * 1000
      break
    case '1m':
      timeDiff = 31 * 24 * 60 * 60 * 1000
      break
    case '6m':
      timeDiff = 6 * 30 * 24 * 60 * 60 * 1000
      break
    case '1y':
      timeDiff = 366 * 24 * 60 * 60 * 1000
      break
    default:
      timeDiff = 24 * 60 * 60 * 1000
      break
  }

  const startTimestamp = Math.floor((now - timeDiff) / 1000)

  let walletAddress = jettonStore.selectedJetton?.jetton.address
  if (jettonStore.selectedJetton?.jetton.symbol == 'TON') walletAddress = 'ton'
  if (walletAddress) {
    points.value = await tonService.getJettonChart(walletAddress, startTimestamp)
    calculatePercentageChange(points.value)
  } else {
    console.error('Wallet address is undefined.')
  }
}

const calculatePercentageChange = (points: Point[]) => {
  let firstElement = points[0].value
  if (selectedPoint.value) {
    firstElement = selectedPoint.value.value
  }
  const lastElement = points[points.length - 1].value
  percentageChange.value = ((firstElement - lastElement) / lastElement) * 100
}

const formatBalance = (balance: string, decimals: number) => {
  return (parseFloat(balance) / Math.pow(10, decimals)).toFixed(2)
}

const calculateJettonBalance = (balance: Balance) => {
  return (
    (parseFloat(balance.balanceAmount) / Math.pow(10, balance.jetton.decimals)) *
    balance.price.prices.usd
  ).toFixed(2)
}

const getColorClass = (value: string) => {
  return value.startsWith('-') ? 'negative' : 'positive'
}

const getDate = (utime: string) => {
  const timestamp = parseInt(utime, 10)
  const date = new Date(timestamp * 1000)
  const months: string[] = [
    'Jan',
    'Feb',
    'Mar',
    'Apr',
    'May',
    'Jun',
    'Jul',
    'Aug',
    'Sep',
    'Oct',
    'Nov',
    'Dec'
  ]

  const day: number = date.getDate()
  const month: string = months[date.getMonth()]
  const hours: string = date.getHours().toString().padStart(2, '0')
  const minutes: string = date.getMinutes().toString().padStart(2, '0')

  const formattedDate: string = `${day} ${month}, ${hours}:${minutes}`

  return formattedDate
}

const jetton = computed(() => jettonStore.selectedJetton)

onMounted(async () => {
  if (jettonStore.selectedJetton?.jetton.symbol == 'TON') {
    jettonHistory.value = await tonService.getTonHistory(userId)
    points.value = await tonService.getJettonChart(
      'ton',
      Math.floor((new Date().getTime() - 24 * 60 * 60 * 1000) / 1000)
    )
    selectedPoint.value = points.value[0]
    calculatePercentageChange(points.value)
    return
  }

  const walletAddress = jettonStore.selectedJetton?.jetton.address
  if (walletAddress) {
    jettonHistory.value = await tonService.getJettonHistory(userId, walletAddress)
    points.value = await tonService.getJettonChart(
      walletAddress,
      Math.floor((new Date().getTime() - 24 * 60 * 60 * 1000) / 1000)
    )

    selectedPoint.value = points.value[0]
    calculatePercentageChange(points.value)
  } else {
    console.error('Wallet address is undefined.')
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

.wallet-container {
  color: white;
  width: 100%;
  height: 80%;
  border-radius: 10px;
  display: flex;
  flex-direction: column;
}

.top-bar {
  display: flex;
  padding: 6px 12px 6px 12px;
  align-items: center;
  margin-bottom: 10px;
  position: sticky;
  top: 0;
  background-color: #10161f;
}

.top-bar::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 1px;
  background: rgba(255, 255, 255, 0.1);
}

.back-button {
  background: none;
  border: none;
  color: white;
  font-size: 20px;
  position: absolute;
  left: 10px;
}

.title {
  margin: 0 auto;
}

.header {
  padding: 5px 20px 5px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.balance-info {
  text-align: left;
}

.balance-info-bottom {
  font-size: 14px;
  font-weight: normal;
  color: #a9bcd4;
}

.balance-info-top {
  font-size: 20px;
  font-weight: bolder;
}

.jetton-icon {
  width: 60px;
  height: 60px;
  border-radius: 50%;
}

.actions {
  display: flex;
  justify-content: space-around;
  margin-bottom: 20px;
}

.actions button {
  background-color: #2e2e2e;
  border: none;
  color: white;
  padding: 10px 20px;
  border-radius: 5px;
}

.price-info {
  padding: 0 20px;
  display: flex;
  flex-direction: column;
  text-align: left;
}

.price-info p {
  font-size: 12px;
}

.chart {
  margin-top: 10px;
  display: flex;
  align-items: center;
  height: 220px;
  margin-bottom: 10px;
  justify-content: center;
  touch-action: none;
  -ms-touch-action: none;
}

.history-list {
  list-style-type: none;
  padding: 0.5rem 0.5rem;
  color: #fff;
  font-size: 16px;
}

.history-item {
  display: flex;
  align-items: center;
  margin-bottom: 10px;
  background-color: #1d2633;
  padding: 5px 10px 5px 10px;
  border-radius: 8px;
}

.left-info {
  display: flex;
  flex-direction: column;
  flex-grow: 1;
}

.right-info {
  text-align: right;
  font-weight: bold;
  display: flex;
  flex-direction: column;
}

.timeframe-controls {
  display: flex;
  justify-content: space-around;
  margin: 10px 0;
}

.timeframe-controls button {
  background: none;
  border: none;
  color: #fff;
  padding: 10px 15px;
  font-size: 16px;
  cursor: pointer;
  transition:
    background 0.3s,
    color 0.3s;
  outline: none !important;
  box-shadow: none !important;
}

.timeframe-controls button.active {
  background-color: #19233c;
  color: #fff;
  border-radius: 5px;
  box-shadow: none;
}
</style>
