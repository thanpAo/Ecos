import { ChatOpenAI } from "@langchain/openai";
import { ChatPromptTemplate, MessagesPlaceholder, } from "@langchain/core/prompts";
import { BufferMemory } from "langchain/memory";
import { LLMChain } from "langchain/chains";

const chatModel = new ChatOpenAI({
    temperature: 0.2,
    openAIApiKey: '',
    modelName: 'gpt-3.5-turbo',
    maxTokens: 200
});

const chatPrompt = ChatPromptTemplate.fromMessages([
    ["system", "You are a nice chatbot having a conversation with a human."],
    new MessagesPlaceholder("chat_history"),
    ["human", "{question}"],
]);

const memory = new BufferMemory({
    memoryKey: 'chat_history',
    returnMessages: true
});

const chatConversationChain = new LLMChain({
    llm: chatModel,
    prompt: chatPrompt,
    verbose: true,
    memory: memory
});

export async function answer(question: string): Promise<string> {
    try {
        const quest = await chatConversationChain.invoke({ question });
        return quest.text;
    } catch (error) {
        console.error("Error al procesar la pregunta:", error);
        throw error;
    }
};

export async function answer3(name: string, flow: Array<[boolean, string]>,
    message: string, params: string, context: string): Promise<string> {
    try {
        const question: string = createPrompt2(name, flow, message, params, context);
        const quest = await chatConversationChain.invoke({ question });
        return quest.text;
    } catch (error) {
        console.error("Error al procesar la pregunta:", error);
        throw error;
    }
}

function createPrompt2(name: string, flow: Array<[boolean, string]>,
    messagePlayer: string, params: string, context: string): string {

    const playerName: string = "Valentine";

    let prompt = `Your name is ${name}. Your personal context is ${context}.
    you are in a theater scene. Write the next line for the script:\n`;

    for (const message of flow) {
        if (message[0]) {
            prompt += `${name}: ${message[1]}.\n`;
        } else {
            prompt += `${playerName}: ${message[1]}.\n`;
        }
    }

    prompt += `${playerName}: ${messagePlayer}.\n`;
    prompt += `${name}: `;
    //prompt += `The last message given by ${playerName} could be: ${params}. But ignore if it´s not realistic\n`;
   /* prompt += `\n. Follow de conversation under the next circunstances:
        Be creative.
        Be original. 
        Dont repeat messages.
        Write only the message. 
        Message under 25 words. 
        Dont write "${name}:"`;*/

    return prompt;
}

export async function answerTest(question: string) {
    return "Tu mensaje fue: " + question;
}