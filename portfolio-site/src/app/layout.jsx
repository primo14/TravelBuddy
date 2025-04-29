import PropTypes from 'prop-types';

export const metadata = {
title: "TravelBuddy",
description: "TravelBuddy is a travel planning app that helps you plan your travel itinerary with ease.",
}

export default function RootLayout({ children }) {
  return (
    <html lang="en">
      <body>
        <div id="root"> {children} </div>
      </body>
    </html>
  );
}

RootLayout.propTypes = {
  children: PropTypes.node.isRequired,
};