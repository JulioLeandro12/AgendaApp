<template>
  <div class="p-4">
    <Toast />
    <ConfirmDialog />

    <div class="flex justify-content-between align-items-center mb-3">
      <h2>Contatos</h2>
      <Button label="Novo Contato" icon="pi pi-plus" class="p-button-success" @click="openNew" />
    </div>

    <DataTable :value="contacts" paginator rows="5" responsiveLayout="scroll">
      <Column field="name" header="Nome"></Column>
      <Column field="email" header="Email"></Column>
      <Column field="phone" header="Telefone"></Column>
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
        <label for="phone">Telefone</label>
        <InputText id="phone" v-model="contact.phone" required />
      </div>

      <template #footer>
        <Button label="Cancelar" icon="pi pi-times" class="p-button-text" @click="hideDialog" />
        <Button :disabled="saving" :loading="saving" label="Salvar" icon="pi pi-check" class="p-button-text" @click="saveContact" />
      </template>
    </Dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Toast from 'primevue/toast'
import ConfirmDialog from 'primevue/confirmdialog'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import { getContacts as fetchContacts, createContact, updateContact, deleteContact as apiDeleteContact } from '../api/api'

// Estados
const contacts = ref([])
const contactDialog = ref(false)
const contact = ref({ id: null, name: '', email: '', phone: '' })
const saving = ref(false)

const toast = useToast()
const confirm = useConfirm()

// Funções principais
const openNew = () => {
  contact.value = { id: null, name: '', email: '', phone: '' }
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
      await createContact(contact.value)
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
