import pic from "../app/assets/cropped-mobile-pic-portfolio.png";
import { motion } from "framer-motion";
const container = (delay) => ({
  hidden: { x: -100, opacity: 0 },
  visible: { x: 0, opacity: 1, transition: { duration: 1, delay: delay } },
});
const Hero = () => {
  return (
    <div className="py-20 pt-48 lg:mb-40">
      <div className="flex flex-wrap ">
        <div className=" xs:max-lg:mx-auto  lg:w-1/2">
          <div className="mt-20 flex flex-col items-center lg:items-start">
            <motion.h1
              variants={container(0.25)}
              initial="hidden"
              whileInView="visible"
              className="md:pb-6  font-thin tracking-tight text-6xl xs:text-3xl sm:text-5xl  xl:mt-20 xl:text-8xl xl:pl-16"
            >
              TravelBuddy
            </motion.h1>
            <motion.div
              variants={container(0.5)}
              initial="hidden"
              whileInView="visible"
              className=" xs:max-sm:w-52 text-center mt-5 lg:max-xl:mt-15 bg-gradient-to-r from-green-300 to-yellow-600 tracking-tight text-transparent  bg-clip-text text-2xl xs:text-base sm:text-xl xl:pl-16"
            >
              Simplify and Declutter Your Trip Planning
            </motion.div>
          </div>
        </div>
        <motion.img
          initial={{ y: 100, opacity: 0 }}
          whileInView={{ y: 0, opacity: 1 }}
          transition={{ duration: 1, delay: 0.5 }}
          src={pic.src}
          className="w-1/2 h-full pt-6 mx-auto"
        />
        <hr></hr>
      </div>
    </div>
  );
};

export default Hero;
