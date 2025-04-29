import { motion } from "framer-motion";
const About = () => {
  return (
    <div className=" pb-24 pt-20 min-w-40 max-w-screen-sm  my-60">
      <motion.h1
        initial={{ y: -100, opacity: 0 }}
        whileInView={{ y: 0, opacity: 1 }}
        transition={{ duration: 1, delay: 0 }} className="mb-10 text-center bg-gradient-to-r from-green-300 to-yellow-700 tracking-tight text-transparent  bg-clip-text text-4xl">
        Reinvent Trip Planning</motion.h1>
        <motion.p initial={{ x: -100, opacity: 0 }}
        whileInView={{ x: 0, opacity: 1 }}
        transition={{ duration: 1, delay: 0.25 }}
        className="mb-5 text-center text-xl leading-10 text-white">  No more juggling multiple apps.</motion.p>
      <motion.p
      initial={{ x: 100, opacity: 0 }}
      whileInView={{ x: 0, opacity: 1 }}
      transition={{ duration: 1, delay: 0.5 }}
       className="text-center text-xl leading-10 text-stone-400">
   
        Whether you're looking up the weather, mapping out your route,
        organizing emails,images and files, or jotting down important details,
        our app consolidates it all. With a user-friendly interface, you can
        plan your entire trip from start to finish, keeping everything organized
        and accessible in one place. Save time, reduce stress, and enjoy a
        smoother travel experience with all the tools you need right at your
        fingertips.
      </motion.p>
    </div>
  );
}


export default About;