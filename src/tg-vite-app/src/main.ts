import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import { useTonConnectStore } from './stores/store'

const app = createApp(App)

app.use(createPinia())
app.use(router)

const tonConnectStore = useTonConnectStore()

async function initializeApp() {
  await tonConnectStore.initializeTonConnectUI()
  app.mount('#app')
}

initializeApp()
