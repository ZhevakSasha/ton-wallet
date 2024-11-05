type Listener = () => void

class EventBus {
  private listeners: { [key: string]: Listener[] } = {}

  on(event: string, callback: Listener) {
    if (!this.listeners[event]) {
      this.listeners[event] = []
    }
    this.listeners[event].push(callback)
  }

  emit(event: string) {
    if (this.listeners[event]) {
      this.listeners[event].forEach((callback) => callback())
    }
  }
}

export const eventBus = new EventBus()
