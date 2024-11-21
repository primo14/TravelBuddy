import Navbar from "./Components/Navbar"
import Hero from "./Components/Hero.jsx"
import Features from "./Components/Features.jsx"
import About from "./Components/About.jsx"
import Download from "./Components/Download.jsx"


const App = () => {
  return (
    <div className="overflow-x-hidden text-neutral-300 antialiased selection:bg-orange-300 selection:text-orange-900">
    <div className="fixed top-0 -z-10 h-full w-full">
      <div className="fixed inset-0 -z-10 h-full w-full bg-white [background:radial-gradient(125%_125%_at_50%_10%,#000_40%,#783f04_100%)]"></div>
    </div>
    
    <div className="container mx-auto px-8">
      <Navbar />
      
      <Hero />
        <div id="Features">
          <Features />
        </div>
        <div id="About">
          <About />
        </div>
        <div id="Download">
          <Download />
        </div>
      </div>
     </div>
    
  )
}

export default App