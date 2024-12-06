import map from "../app/assets/map.svg"
import weather from "../app/assets/weather.svg"
import file from "../app/assets/file.svg"
import location from "../app/assets/location.svg"
import plan from "../app/assets/plan.svg"
import { motion } from "framer-motion"
const iconMap = {
  map,
  weather,
  file,
  location,
  plan
}
const container = (delay, index) => ({
    hidden: { x: index > 3 ? -100 : 100, opacity: 0 },
    visible: { x: 0, opacity: 1, transition: { duration: 1, delay: delay } },
  });
const FeatureCard = ({feature}) => {
  return (
    <motion.div 
    variants={container((feature.id * 0.01),feature.id)}
    initial="hidden"
    whileInView="visible"
    className="rounded-2xl bg-stone-950 p-6 w-1/4 flex flex-col items-center">
        <img src={iconMap[feature.icon].src}/>
        <h1 className="text-center py-4 font-semibold">{feature.title}</h1>
        
        <p className="text-center text-stone-400 xl:h-28 lg:h-20 sm:h-64 md:h-48">{feature.description}</p>
    </motion.div>
  )
}

export default FeatureCard