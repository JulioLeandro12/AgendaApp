import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

// PrimeVue and CSS
import PrimeVue from 'primevue/config'
import 'primevue/resources/themes/lara-light-blue/theme.css'
import 'primevue/resources/primevue.min.css'
import 'primeicons/primeicons.css'

// Additional PrimeVue services
import ToastService from 'primevue/toastservice'
import ConfirmationService from 'primevue/confirmationservice'

// Create app instance
const app = createApp(App)

app.use(router)
app.use(PrimeVue)
app.use(ToastService)
app.use(ConfirmationService)

app.mount('#app')
