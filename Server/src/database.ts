import mongoose from 'mongoose';

const connectDB = async (): Promise<void> => {
    try {
        const mongoURL = "mongodb+srv://EcosTeam:Z8sQ1yj2K93zIb2F@ecos.onfoxzk.mongodb.net/?retryWrites=true&w=majority&appName=Ecos";

        if (!mongoURL) {
            throw new Error('MONGO_URI is not defined');
        }

        await mongoose.connect(mongoURL);

        console.log('MongoDB connected successfully');
    } catch (error) {
        console.error('Error connecting to MongoDB:', error);
        //process.exit(1);
    }
};

export default connectDB;