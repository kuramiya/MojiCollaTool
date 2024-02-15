import re

def extract_characters(input_string, start_string, end_string):
    pattern = re.compile(start_string + '(.*?)' + end_string)
    matches = re.findall(pattern, input_string)
    return matches

# Function to read input string from file
def read_input_string_from_file(file_name):
    with open(file_name, 'r') as file:
        return file.read()

# Function to write output to file
def write_output_to_file(file_name, specific_text, output_list):
    with open(file_name, 'r') as file:
        lines = file.readlines()

    with open(file_name, 'w') as file:
        for line in lines:
            file.write(line)
            if line.strip() == specific_text:
                for item in output_list:
                    file.write("%s\n" % item)

# Example usage:
input_file_name = "colorsource.html"  # Update with your input file name
output_file_name = "output.txt"  # Update with your desired output file name
specific_text = "Specific text to append results to:"  # Update with the specific text

start_string = "--bgc:" 
end_string = "--tc:"

input_string = read_input_string_from_file(input_file_name)
enclosed_texts = extract_characters(input_string, start_string, end_string)

write_output_to_file(output_file_name, specific_text, enclosed_texts)
print("Output has been written to", output_file_name)
