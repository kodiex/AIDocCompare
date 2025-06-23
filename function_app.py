import azure.functions as func
import logging
from openai import OpenAI

app = func.FunctionApp(http_auth_level=func.AuthLevel.FUNCTION)

@app.route(route="ComparePDF")
def ComparePDF(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Processing Base64-encoded file.')

    try:
        # Get the Base64-encoded file from the request
        base64_file = req.params.get('file')
        
        # Handle file parameter in POST request
        if not base64_file:
            try:
                req_body = req.get_json()
            except ValueError:
                return func.HttpResponse(
                    "Invalid JSON in request body.",
                    status_code=400
                )
            else:
                base64_file = req_body.get('file')

        if not base64_file:
            return func.HttpResponse(
                "Missing 'file' parameter.",
                status_code=400
            )

        # Correct OpenAI client usage
        openai_client = OpenAI(api_key="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")
        

       
        completion = openai_client.chat.completions.create(
            model="gpt-4.1",
            messages=[
                {
                    "role": "user",
                    "content": [
                        {
                            "type": "file",
                            "file": {
                                "filename": "draconomicon.pdf",
                                "file_data": f"data:application/pdf;base64,{base64_file}",
                            }
                        },
                        {
                            "type": "text",
                            "text": "What is the first dragon in the book?",
                        }
                    ],
                },
            ],
        )

        return func.HttpResponse(
            f"File sent to OpenAI successfully. Response: {completion.choices[0].message.content}",
            status_code=200
        )
       
    except Exception as e:
        logging.error(f"Unexpected error: {e}")
        print(f"Unexpected error: {e}")
        return func.HttpResponse(
            f"Internal server error: {str(e)}",
            status_code=500
        )