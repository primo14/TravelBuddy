import { motion } from "framer-motion";

const Download = () => {
  return (
    <div className=" pb-24 pt-20 mx-60 my-60 flex flex-col items-center ">
      <motion.h1
        initial={{ y: -100, opacity: 0 }}
        whileInView={{ y: 0, opacity: 1 }}
        transition={{ duration: 1, delay: 0 }}
        className="my-10 text-center bg-gradient-to-r from-green-300 to-yellow-700 tracking-tight text-transparent  bg-clip-text text-4xl"
      >
        Download Trial
      </motion.h1>
      <motion.p
        initial={{ x: -100, opacity: 0 }}
        whileInView={{ x: 0, opacity: 1 }}
        transition={{ duration: 1, delay: 0.25 }}
        className="text-center text-xl leading-10 text-stone-400"
      >
        We will be releasing the full version of OCD Traveller soon.
      </motion.p>
      <motion.p
        initial={{ x: -100, opacity: 0 }}
        whileInView={{ x: 0, opacity: 1 }}
        transition={{ duration: 1, delay: 0.25 }}
        className="text-center text-xl leading-10 text-stone-400"
      >
        In the meantime, you can download a trial version of the app to get a
        feel for its features and functionality.
      </motion.p>
      <a href="/Travel_App/Trip_Planner.apk" download >
        <motion.button
          initial={{ x: 100, opacity: 0 }}
          whileInView={{ x: 0, opacity: 1 }}
          transition={{ duration: 1, delay: 0.5 }}
          className="border-2 border-white bg-gradient-to-b from-black from-60% to-stone-950 text-white font-bold py-4 px-6 rounded-2xl mt-10"
        >
          Download Trial
        </motion.button>
      </a>
    </div>
  );
};

export default Download;
