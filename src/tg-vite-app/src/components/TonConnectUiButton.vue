<template>
  <div id="connect"></div>
</template>

<script setup lang="ts">
import { useTonConnectStore } from '@/stores/store'
import { TonConnectUI } from '@tonconnect/ui'
import { nextTick, onMounted } from 'vue'

const tonConnectStore = useTonConnectStore()
onMounted(async () => {
  const tonConnectUI = tonConnectStore.tonConnectUI
  if (!tonConnectUI) await tonConnectStore.initializeTonConnectUI()
})
</script>
