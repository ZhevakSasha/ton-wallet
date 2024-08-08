import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'
import { resolve } from 'path'
import fs from 'fs'
import { createHtmlPlugin } from 'vite-plugin-html'

const key = fs.readFileSync(resolve(__dirname, 'key.pem'))
const cert = fs.readFileSync(resolve(__dirname, 'cert.pem'))

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vueDevTools(),
    createHtmlPlugin({
      inject: {
        data: {
          script: `<script src="https://telegram.org/js/telegram-web-app.js"></script>`
        }
      }
    })
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  server: {
    https: {
      key,
      cert
    },
    port: 4444
  }
})
