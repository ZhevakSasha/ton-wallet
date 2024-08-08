<template>
  <div class="chart-container">
    <canvas ref="chartCanvas"></canvas>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { Chart, registerables, type ChartConfiguration, type ChartEvent } from 'chart.js'
import type { Point } from '@/services/types'

Chart.register(...registerables)

const props = defineProps<{ points: Point[]; selectedPeriod: string }>()

const chartCanvas = ref<HTMLCanvasElement | null>(null)
let chart: Chart | null = null
let verticalLinePosition: number | null = null

const emit = defineEmits<{
  (event: 'point-selected', value: Point | null): void
}>()

onMounted(() => {
  createChart()
})

watch(
  () => props.points,
  () => {
    if (chart) {
      updateChart()
    }
  }
)

const createChart = () => {
  if (!chartCanvas.value) return

  const sortedPoints = [...props.points].sort((a, b) => a.utime - b.utime)

  const ctx = chartCanvas.value.getContext('2d')
  if (!ctx) return
  const gradient = ctx.createLinearGradient(0, 0, 0, chartCanvas.value.height)
  gradient.addColorStop(0, 'rgba(51, 161, 253, 0.3)')
  gradient.addColorStop(1, 'rgba(51, 161, 253, 0)')
  const data = {
    labels: sortedPoints.map((point) => formatDate(point.utime, props.selectedPeriod)),
    datasets: [
      {
        label: 'Jetton Value',
        data: sortedPoints.map((point) => point.value),
        borderColor: '#33A1FD',
        backgroundColor: gradient,
        fill: true,
        tension: 0.1,
        pointRadius: 0,
        pointHoverRadius: 8
      }
    ]
  }

  const minValue = Math.min(...sortedPoints.map((point) => point.value))
  const maxValue = Math.max(...sortedPoints.map((point) => point.value))
  const range = maxValue - minValue
  const suggestedMin = minValue - range * 0.1 // Добавляем 10% снизу
  const suggestedMax = maxValue + range * 0.1 // Добавляем 10% сверху

  const config: ChartConfiguration<'line', number[], string> = {
    type: 'line',
    data,
    options: {
      responsive: true,
      layout: {
        padding: {
          left: -10
        }
      },
      maintainAspectRatio: false,
      scales: {
        x: {
          type: 'category',
          display: true,
          title: {
            display: false,
            text: 'Time',
            color: '#999999',
            font: {
              size: 14
            }
          },
          ticks: {
            color: '#999999',
            font: {
              size: 12
            },
            maxTicksLimit: 2,
            align: 'start',
            callback: function (value: string, index: number, values: string[]) {
              const utime = this.getLabelForValue(index)
              return formatDate(parseFloat(utime), props.selectedPeriod)
            }
          }
        },
        y: {
          display: false,
          title: {
            display: false,
            text: 'Value',
            color: '#999999',
            font: {
              size: 14
            }
          },
          ticks: {
            display: false,
            color: '#999999',
            font: {
              size: 12
            },
            maxTicksLimit: 2
          }
        }
      },
      plugins: {
        legend: {
          display: false
        },
        tooltip: {
          enabled: false,
          callbacks: {
            label: function (context) {
              let label = context.dataset.label || ''
              if (label) {
                label += ': '
              }
              const value = context.raw as number
              if (value !== null && value !== undefined) {
                label += value.toFixed(4)
              }
              return label
            }
          }
        }
      },
      interaction: {
        mode: 'none',
        intersect: false,
        axis: 'x'
      },
      onHover: (event: ChartEvent, chartElement: ChartElement[]) => {
        if (chartElement.length) {
          const index = chartElement[0].index
          const value = chart.data.datasets[0].data[index]

          const point: Point = {
            value: value,
            utime: chart.data.labels[index]
          }
          console.log(point.value)
          emit('point-selected', point)
        }
      }
    },
    plugins: [
      {
        id: 'verticalLine',
        afterDraw: (chart) => {
          if (verticalLinePosition !== null) {
            const ctx = chart.ctx
            const x = chart.scales.x.getPixelForValue(verticalLinePosition)
            const yAxis = chart.scales.y
            ctx.save()
            ctx.beginPath()
            ctx.moveTo(x, yAxis.top)
            ctx.lineTo(x, yAxis.bottom)
            ctx.lineWidth = 1
            ctx.strokeStyle = '#33A1FD'
            ctx.stroke()
            ctx.restore()
          }
        }
      }
    ]
  }

  chart = new Chart(chartCanvas.value, config)
  chartCanvas.value.addEventListener('mousedown', (event) => setVerticalLine(event))
  chartCanvas.value.addEventListener('mouseup', () => clearVerticalLine())
  chartCanvas.value.addEventListener('mousemove', (event) => throttledUpdateVerticalLine(event))
  chartCanvas.value.addEventListener('touchstart', (event) => setVerticalLine(event))
  chartCanvas.value.addEventListener('touchend', () => clearVerticalLine())
  chartCanvas.value.addEventListener('touchmove', (event) => throttledUpdateVerticalLine(event))
}

const setVerticalLine = (event: MouseEvent | TouchEvent) => {
  if (!chart || !chartCanvas.value) return
  chart.options.interaction = {
    mode: 'nearest',
    intersect: false,
    axis: 'x'
  }
  const x =
    event instanceof MouseEvent
      ? event.offsetX
      : event.touches[0].clientX - chartCanvas.value.getBoundingClientRect().left
  verticalLinePosition = chart.scales.x.getValueForPixel(x) as number
  chart.update()
}

const updateVerticalLine = (event: MouseEvent | TouchEvent) => {
  if (!verticalLinePosition) return
  setVerticalLine(event)
}

const clearVerticalLine = () => {
  verticalLinePosition = null
  if (chart) {
    emit('point-selected', props.points[0])
    chart.options.interaction = {
      mode: 'none'
    }
    chart.update()
  }
}

const updateChart = () => {
  if (!chart) return

  const sortedPoints = [...props.points].sort((a, b) => a.utime - b.utime)

  chart.data.labels = sortedPoints.map((point) => point.utime)
  chart.data.datasets[0].data = sortedPoints.map((point) => point.value)
  chart.update()
}

const formatDate = (utime: number, period: string) => {
  const date = new Date(utime * 1000)
  if (period === '1h' || period === '1d') {
    const hours = date.getHours().toString().padStart(2, '0')
    const minutes = date.getMinutes().toString().padStart(2, '0')
    return `${hours}:${minutes}`
  } else {
    const day = date.getDate().toString().padStart(2, '0')
    const month = date.toLocaleString('en-US', { month: 'short' })
    return `${day} ${month}.`
  }
}

function throttle(func: (...args: any[]) => void, limit: number) {
  let lastFunc: ReturnType<typeof setTimeout>
  let lastRan: number
  return function (...args: any[]) {
    if (!lastRan) {
      func.apply(this, args)
      lastRan = Date.now()
    } else {
      clearTimeout(lastFunc)
      lastFunc = setTimeout(
        function () {
          if (Date.now() - lastRan >= limit) {
            func.apply(this, args)
            lastRan = Date.now()
          }
        },
        limit - (Date.now() - lastRan)
      )
    }
  }
}

const throttledUpdateVerticalLine = throttle(updateVerticalLine, 10)
</script>

<style scoped>
.chart-container {
  width: 100%;
  height: 100%;
}

canvas {
  max-width: 100%;
  height: 100%;
}
</style>
