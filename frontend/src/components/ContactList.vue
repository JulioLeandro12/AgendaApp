<template>
  <div class="p-4">
    <Toast />
    <ConfirmDialog />

    <div class="flex justify-content-between align-items-center mb-3">
      <h2>Contatos</h2>
      <Button label="Novo Contato" icon="pi pi-plus" class="p-button-success" @click="openNew" />
    </div>

    <DataTable :value="contacts" paginator :rows="5" :rowsPerPageOptions="[5]" responsiveLayout="scroll">
      <Column field="name" header="Nome"></Column>
      <Column field="email" header="Email"></Column>
      <Column header="Telefone">
        <template #body="slotProps">
          {{ formatPhoneTable(slotProps.data.phone) }}
        </template>
      </Column>
      <Column header="Ações">
        <template #body="slotProps">
          <Button icon="pi pi-pencil" class="p-button-text p-button-rounded p-button-warning" @click="editContact(slotProps.data)" />
          <Button icon="pi pi-trash" class="p-button-text p-button-rounded p-button-danger" @click="confirmDeleteContact(slotProps.data)" />
        </template>
      </Column>
    </DataTable>

    <Dialog v-model:visible="contactDialog" header="Adicionar Contato" :modal="true" class="p-fluid">
      <div class="field">
        <label for="name">Nome</label>
        <InputText id="name" v-model="contact.name" required autofocus />
      </div>

      <div class="field">
        <label for="email">Email</label>
        <InputText id="email" v-model="contact.email" required />
      </div>

      <div class="field">
        <label for="country">País</label>
        <Dropdown id="country" v-model="selectedCountry" :options="countries" optionLabel="label" class="w-full" />
      </div>

      <div class="field">
        <label for="phone">Telefone</label>
        <InputText id="phone" v-model="phoneDisplay" required :placeholder="phonePlaceholder" />
      </div>

      <template #footer>
        <Button label="Cancelar" icon="pi pi-times" class="p-button-text" @click="hideDialog" />
        <Button :disabled="saving" :loading="saving" label="Salvar" icon="pi pi-check" class="p-button-text" @click="saveContact" />
      </template>
    </Dialog>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Toast from 'primevue/toast'
import ConfirmDialog from 'primevue/confirmdialog'
import Dropdown from 'primevue/dropdown'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import { getContacts as fetchContacts, createContact, updateContact, deleteContact as apiDeleteContact } from '../api/api'

// states
const contacts = ref([])
const contactDialog = ref(false)
const contact = ref({ id: null, name: '', email: '', phone: '' })
const saving = ref(false)

const toast = useToast()
const confirm = useConfirm()

// formatation 
const countries = [
  { code: 'BR', dial: '55', label: 'Brasil (+55)' },
  { code: 'US', dial: '1', label: 'Estados Unidos (+1)' },
  { code: 'PT', dial: '351', label: 'Portugal (+351)' }
]
const selectedCountry = ref(countries[0])

// Functions for formatting by country
const formatByCountry = (dial, national) => {
  if (!national) return `+${dial}`
  if (dial === '55') {
    // BR: DDD(2) + 8/9 digits
    if (national.length < 10) return `+55 ${national}`
    const ddd = national.slice(0, 2)
    if (national.length >= 11) {
      return `+55 ${ddd} ${national.slice(2, 7)}-${national.slice(7, 11)}`
    } else {
      return `+55 ${ddd} ${national.slice(2, 6)}-${national.slice(6, 10)}`
    }
  }
  if (dial === '1') {
    // US: 10 digits: AAA NNN-NNNN
    if (national.length < 10) return `+1 ${national}`
    return `+1 ${national.slice(0,3)} ${national.slice(3,6)}-${national.slice(6,10)}`
  }
  if (dial === '351') {
    // PT: 9 digits: 9XX XXX XXX
    if (national.length < 9) return `+351 ${national}`
    return `+351 ${national.slice(0,3)} ${national.slice(3,6)} ${national.slice(6,9)}`
  }
  // Generic: groups in blocks of 3-4
  if (national.length <= 4) return `+${dial} ${national}`
  if (national.length <= 7) return `+${dial} ${national.slice(0,3)} ${national.slice(3)}`
  return `+${dial} ${national.slice(0,3)} ${national.slice(3,7)}-${national.slice(7,11)}`
}

const extractDialAndNational = (raw) => {
  const digits = String(raw || '').replace(/\D/g, '')
  if (!digits) return { dial: selectedCountry.value.dial, national: '' }
  // tries to match known DDI (prefer the longest)
  const dials = countries.map(c => c.dial).sort((a,b) => b.length - a.length)
  for (const d of dials) {
    if (digits.startsWith(d)) {
      return { dial: d, national: digits.slice(d.length) }
    }
  }
  // default: use selected country and consider everything as national
  return { dial: selectedCountry.value.dial, national: digits }
}

const formatPhone = (raw) => {
  const { dial, national } = extractDialAndNational(raw)
  return formatByCountry(dial, national)
}

// Display in table: does not depend on the country selected in the form.
// If no recognized DDI, shows the original value (just trimmed) to avoid "changing" the country visually.
const formatPhoneTable = (raw) => {
  const digits = String(raw || '').replace(/\D/g, '')
  if (!digits) return ''
  const known = countries.map(c => c.dial).sort((a,b) => b.length - a.length)
  for (const d of known) {
    if (digits.startsWith(d)) {
      const national = digits.slice(d.length)
      return formatByCountry(d, national)
    }
  }
  // If no recognized DDI, returns the original value without changing the country
  return String(raw || '').trim()
}

// Computed to mask the input while keeping only digits in the state
const phoneDisplay = computed({
  get() {
    return formatPhone(contact.value.phone)
  },
  set(v) {
    const rawDigits = String(v || '').replace(/\D/g, '')
    if (!rawDigits) { contact.value.phone = ''; return }
    // separates if the user typed with +code in the field
    let dial = selectedCountry.value.dial
    let national = rawDigits
    if (rawDigits.startsWith(dial)) {
      national = rawDigits.slice(dial.length)
    }
    // applies limits by country
    if (dial === '55') national = national.slice(0, 11) // DDD(2)+8/9
    else if (dial === '1') national = national.slice(0, 10) // US
    else if (dial === '351') national = national.slice(0, 9) // PT
    else national = national.slice(0, 12)
    contact.value.phone = '+' + dial + national
  }
})

const phonePlaceholder = computed(() => {
  const d = selectedCountry.value.dial
  if (d === '55') return '+55 81 986941088'
  if (d === '1') return '+1 212 5551234'
  if (d === '351') return '+351 912 345678'
  return `+${d} ...`
})

// Main functions
const openNew = () => {
  // Do not include id to avoid sending id:null (breaks int binding on backend)
  contact.value = { name: '', email: '', phone: '' }
  selectedCountry.value = countries[0]
  contactDialog.value = true
}

const hideDialog = () => {
  contactDialog.value = false
}

const loadContacts = async () => {
  contacts.value = await fetchContacts()
}

const saveContact = async () => {
  saving.value = true
  try {
    if (contact.value.id) {
      await updateContact(contact.value.id, contact.value)
      toast.add({ severity: 'success', summary: 'Atualizado!', detail: 'Contato atualizado com sucesso.', life: 3000 })
    } else {
      // Remove id in case some residual state has it, avoiding id:null in the payload
      const newPayload = { ...contact.value }
      delete newPayload.id
      await createContact(newPayload)
      toast.add({ severity: 'success', summary: 'Adicionado!', detail: 'Contato adicionado com sucesso.', life: 3000 })
    }
    contactDialog.value = false
    await loadContacts()
  } catch (err) {
    const msgs = err.normalizedMessages || ['Falha ao salvar contato.']
    msgs.forEach(m => toast.add({ severity: 'error', summary: 'Erro', detail: m, life: 4000 }))
  } finally {
    saving.value = false
  }
}

const editContact = (data) => {
  contact.value = { ...data }
  // Detect country from existing number
  const digits = String(data.phone || '').replace(/\D/g, '')
  const found = countries.find(c => digits.startsWith(c.dial))
  if (found) selectedCountry.value = found
  contactDialog.value = true
}

const confirmDeleteContact = (data) => {
  confirm.require({
    message: `Deseja excluir o contato "${data.name}"?`,
    header: 'Confirmação',
    icon: 'pi pi-exclamation-triangle',
    accept: async () => {
      await apiDeleteContact(data.id)
      toast.add({ severity: 'info', summary: 'Excluído', detail: 'Contato removido.', life: 3000 })
      await loadContacts()
    }
  })
}

onMounted(loadContacts)
</script>

<style scoped>
.field {
  margin-bottom: 1rem;
}
</style>
