<template>
  <div class="app-container">
    <Menubar :model="items" />

    <div class="p-4">
      <router-view></router-view>
    </div>
  </div>
</template>

<script setup>
import Menubar from 'primevue/menubar'

const items = [
  {
    label: 'Início',
    icon: 'pi pi-home',
    command: () => window.location.href = '/'
  },
  {
    label: 'Contatos',
    icon: 'pi pi-users',
    command: () => window.location.href = '/contacts'
  },
  {
    label: 'Sobre',
    icon: 'pi pi-info-circle',
    command: () => window.location.href = '/about'
  }
]
</script>
