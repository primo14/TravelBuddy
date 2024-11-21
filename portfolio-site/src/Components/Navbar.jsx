import { motion } from "framer-motion";
import logo from "../assets/navigation_icon.svg"
const Navbar = () => {
  return (
    <motion.div
      initial={{ y: -100, opacity: 0 }}
      animate={{ y: 0, opacity: 1 }}
      transition={{ duration: 0.5, delay: 0.25 }}
      className="fixed top-0 w-full bg-gradient-to-b  from-black from-50% z-50"
    >
      <nav className="ml-5 flex items-center justify-between py-4">
        <div className="flex flex-shrink-0 items-center">
          <a href="/"><img src={ logo }/></a>
        </div>
        <div className="m-8 mr-48 flex items-center justify-center gap-10">
          <h3 className=" text-white">
            <a href="#Features">Features</a>
          </h3>
          <h3 className=" text-white">
            <a href="#About">About</a>
          </h3>
          <h3 className=" text-white">
            <a href="#Download">Download</a>
          </h3>
          <h3 className=" text-white">
            <a href="https://github.com/primo14/TravelBuddy">Github</a>
          </h3>
        </div>
      </nav>
    </motion.div>
  );
};

export default Navbar;
