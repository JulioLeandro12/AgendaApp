import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '/api'
})

export async function getContacts() {
  const res = await api.get('/contacts');
  return res.data;
}

export async function getContact(id) {
  const res = await api.get(`/contacts/${id}`);
  return res.data;
}

export async function createContact(payload) {
  const res = await api.post('/contacts', payload);
  return res.data;
}

export async function updateContact(id, payload) {
  await api.put(`/contacts/${id}`, payload);
}

export async function deleteContact(id) {
  await api.delete(`/contacts/${id}`);
}
