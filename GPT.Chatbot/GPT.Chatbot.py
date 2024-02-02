
import os
import openai

# Set your OpenAI API key here
config = Config()

openai.api_key = os.getenv("API_KEY")


openai.File.create(
  file=open("mydata.jsonl", "rb"),
  purpose='fine-tune'
)

openai.FineTuningJob.create(training_file="file-abc123", model="gpt-3.5-turbo")


completion = openai.ChatCompletion.create(
  model="ft:gpt-3.5-turbo:my-org:custom_suffix:id",
  messages=[
    {"role": "system", "content": "You are a helpful assistant."},
    {"role": "user", "content": "Hello!"}
  ]
)

print(completion.choices[0].message)

