export interface Price {
  prices: {
    usd: number
    uah: number
  }
  diff24h: {
    usd: string
    uah: string | null
  }
  diff7d: {
    usd: string
    uah: string | null
  }
  diff30d: {
    usd: string
    uah: string | null
  }
}

export interface WalletAddress {
  address: string
  isScam: boolean
  isWallet: boolean
}

export interface Jetton {
  address: string
  name: string
  symbol: string
  decimals: number
  image: string
  verification: string
}

export interface Balance {
  balanceAmount: string
  price: Price
  walletAddress: WalletAddress
  jetton: Jetton
}

export interface TransactionHistory {
  utime: string
  transactionType: TransactionType
  comission: number
  value: number
}

export interface Point {
  utime: number
  value: number
}

export enum TransactionType {
  Sent = 'Sent',
  Received = 'Received',
  Exchange = 'Exchange'
}
