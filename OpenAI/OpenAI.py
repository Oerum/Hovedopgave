import openai

openai.api_key = "null"

def Create_fine_tune_upload_file():
    #Upload the training file
    file_response = openai.File.create(
        file=open("json.jsonl", "rb"),
        purpose='fine-tune'
    )
    
    return file_response

def create_fine_tuning_job(training_file_id, model="gpt-3.5-turbo"):
    # Create a FineTuningJob
    fine_tuning_response = openai.FineTuningJob.create(
        training_file=training_file_id,
        model=model
    )
    
    return fine_tuning_response

def delete_files():
    files = openai.File.list()

    for file in files.data:
        file_id = file.id
        openai.File.delete(file_id)
        print(f"Deleted file with ID: {file_id}")



print(openai.FineTuningJob.list())

#print(Create_fine_tune_upload_file())

#delete_files()

print(openai.File.list())

#print(create_fine_tuning_job("file-Kb4ToHrhl3pWMdZN7yGJ4N6b"))
