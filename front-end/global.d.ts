declare global {
  interface Window {
    electron: {
      send: (channel: string, data: unknown) => void;
      onMessage: (channel: string, callback: (event, data: unknown) => void) => void;
      removeAllListeners: (channel: string) => void;
    };
  }
}

export { };