<template>
  <div class="welcome-container">
    <div class="welcome-items">
      <div class="welcome">Welcome</div>
    </div>
    <TonConnectUiButton></TonConnectUiButton>
  </div>
</template>

<script setup lang="ts">
import TonConnectUiButton from '@/components/TonConnectUiButton.vue'
</script>

<style scoped>
.welcome-container {
  padding-top: 15vh;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  height: 90vh;
  color: white;
  box-sizing: border-box;
}

.welcome {
  font-size: 40px;
}
</style>
