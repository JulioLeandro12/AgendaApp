import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

// PrimeVue e CSS
import PrimeVue from 'primevue/config'
import 'primevue/resources/themes/lara-light-blue/theme.css'
import 'primevue/resources/primevue.min.css'
import 'primeicons/primeicons.css'

// Serviços adicionais do PrimeVue
import ToastService from 'primevue/toastservice'
import ConfirmationService from 'primevue/confirmationservice'

// Criação da instância do app
const app = createApp(App)

app.use(router)
app.use(PrimeVue)
app.use(ToastService)
app.use(ConfirmationService)

app.mount('#app')
