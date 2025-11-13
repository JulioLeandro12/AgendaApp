import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5000/api/Contacts',
})

export default {
  getAll() {
    return api.get('/')
  },
  create(contact) {
    return api.post('/', contact)
  },
  update(id, contact) {
    return api.put(`/${id}`, contact)
  },
  delete(id) {
    return api.delete(`/${id}`)
  },
}
