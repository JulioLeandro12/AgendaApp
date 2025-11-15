import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '/api'
})

// Interceptor para normalizar mensagens de erro (retorna array de mensagens ou string)
api.interceptors.response.use(
  r => r,
  err => {
    if (err.response && err.response.data) {
      const data = err.response.data
      // FluentValidation ProblemDetails: data.errors = { field: [msg] }
      if (data.errors) {
        const msgs = Object.values(data.errors).flat()
        err.normalizedMessages = msgs
      } else if (data.title || data.detail) {
        err.normalizedMessages = [data.detail || data.title]
      }
    }
    return Promise.reject(err)
  }
)

export async function getContacts() {
  const { data } = await api.get('/contacts')
  return data
}

export async function getContact(id) {
  const { data } = await api.get(`/contacts/${id}`)
  return data
}

export async function createContact(payload) {
  const { data } = await api.post('/contacts', payload)
  return data
}

export async function updateContact(id, payload) {
  await api.put(`/contacts/${id}`, payload)
}

export async function deleteContact(id) {
  await api.delete(`/contacts/${id}`)
}

export { api }
