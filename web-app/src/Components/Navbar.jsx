import { motion } from "framer-motion";
import logo from "../app/assets/navigation_logo.svg";
import menu_icon from "../app/assets/menu_icon.svg"
const Navbar = () => {
  function toggleMenu(){
    console.log("menu")
    
  }

  return (
    <motion.div
      initial={{ y: -100, opacity: 0 }}
      animate={{ y: 0, opacity: 1 }}
      transition={{ duration: 0.5, delay: 0.25 }}
      className="container  fixed top-0 bg-gradient-to-b  from-black from-50% z-50"
    >
      <nav className=" ml-5 flex items-center justify-between py-4">
        <div className="sm:ml-10 xs:max-sm:ml-2 flex flex-shrink-0 items-center">
          <a href="/"><img src={ logo.src }/></a>
        </div>
        <div className="m-8 mr-30 flex items-center justify-center   xs: max-md:pr-5 sm:gap-10 xs:max-sm:gap-4">
          <h3 className=" text-white xs:max-sm:text-xs xs:max-sm:hidden">
            <a href="#Features">Features</a>
          </h3>
          <h3 className=" text-white xs:max-sm:text-xs xs:max-sm:hidden">
            <a href="#About">About</a>
          </h3>
          <h3 className=" text-white xs:max-sm:text-xs xs:max-sm:hidden">
            <a href="#Download">Download</a>
          </h3>
          <h3 className=" text-white xs:max-sm:text-xs xs:max-sm:hidden">
            <a href="https://github.com/primo14/TravelBuddy">Github</a>
          </h3>
        </div>
        <button id="side_menu_button" className="sm:hidden" onClick={toggleMenu}>
          <img className = "w-9"src={ menu_icon.src }/>
        </button>
      </nav>
    </motion.div>
  );
};

export default Navbar;
