<template>
  <div class="p-fluid">
    <div class="p-field">
      <label for="name">Nome</label>
      <InputText id="name" v-model="contact.name" required autofocus />
    </div>

    <div class="p-field">
      <label for="email">E-mail</label>
      <InputText id="email" v-model="contact.email" type="email" />
    </div>

    <div class="p-field">
      <label for="phone">Telefone</label>
      <InputText id="phone" v-model="contact.phone" />
    </div>

    <div class="p-d-flex p-jc-end p-mt-3">
      <Button label="Cancelar" icon="pi pi-times" class="p-button-text" @click="$emit('cancel')" />
      <Button label="Salvar" icon="pi pi-check" class="p-ml-2" @click="emitSave" />
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'

const props = defineProps({ contact: Object })
const emit = defineEmits(['save', 'cancel'])
const contact = ref({ ...props.contact })

watch(() => props.contact, (newVal) => {
  contact.value = { ...newVal }
})

const emitSave = () => {
  emit('save', contact.value)
}
</script>
