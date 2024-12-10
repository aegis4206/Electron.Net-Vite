import { useEffect, useRef } from 'react'
import './App.css'


function App() {
  const first = useRef(0)


  useEffect(() => {
    if (first.current == 0) {
      window.electron.onMessage('back-end', handleMessage);
    }

    first.current = 1

    return ()=>{
      window.electron.removeAllListeners('back-end');
      console.log("remove listeners");
    }
  }, [])

  const handleMessage = (event: unknown, data: unknown) => {
    console.log(event);
    console.log(data);
  };

  return (
    <>
      <div>Demo</div>
    </>
  )
}

export default App
