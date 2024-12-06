import FeatureCard from "./FeatureCard";
import { FeatureList } from "../app/assets/Inputs";
import { motion } from "framer-motion";


const Features = () => {
  return (
    <div className="pt-48 pb-24 ">
      <motion.h1
        initial={{ y: -100, opacity: 0 }}
        whileInView={{ y: 0, opacity: 1 }}
        transition={{ duration: 1, delay: 0 }}
        className=" pb-20 text-center bg-gradient-to-r from-green-300 to-yellow-700 tracking-tight text-transparent  bg-clip-text text-4xl "
      >
        Features
      </motion.h1>
      <div className="flex flex-wrap items-center justify-center gap-4">
        {FeatureList.map((feature, index) => (
          
            <FeatureCard key={index} feature={feature} />
          
        ))}
      </div>
    </div>
  );
};

export default Features;
