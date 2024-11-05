<template>
  <div class="wrapper">
    <router-view v-slot="{ Component }">
      <transition name="slide-right" mode="out-in">
        <component :is="Component" />
      </transition>
    </router-view>
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { eventBus } from './services/eventBus'
import { useTonConnectStore } from '@/stores/store'
import { RouterLink, RouterView } from 'vue-router'

const router = useRouter()
const store = useTonConnectStore()

const handleAuthError = () => {
  localStorage.removeItem('accessToken')
  localStorage.removeItem('refreshToken')
  store.disconnect()
  router.push({ name: 'welcome' })
}

onMounted(() => {
  eventBus.on('authError', handleAuthError)
})
</script>

<style>
.slide-right-enter-active,
.slide-right-leave-active {
  transition: transform 0.3s ease;
}
.slide-right-enter-from {
  transform: translateX(-100%);
}
.slide-right-leave-to {
  transform: translateX(100%);
}
</style>
