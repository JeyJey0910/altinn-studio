import path from 'path';
import fs from 'fs';
import Ajv2020 from 'ajv/dist/2020';
import type { JsonSchemaNode } from '../src/lib/types';
import { FieldType, Keywords } from '../src';

/**
 * Some schemas might not be valid
 */
export const ignoreTestSchemas: string[] = ['ComplexSchema-old'];

const defaultPath = path.resolve(__dirname, '../../../../DataModeling.Tests/_TestData/Model/JsonSchema');

const cache = new Map();

const readJsonFile = (filepath): JsonSchemaNode => JSON.parse(fs.readFileSync(filepath, 'utf-8'));
/**
 * Returns a map with json schemas.
 */
const getJsonSchemasForTest = (dirPath: string): Map<string, JsonSchemaNode> => {
  if (!cache.has(dirPath)) {
    const output = new Map<string, JsonSchemaNode>();
    fs.readdirSync(dirPath).forEach((filename) => {
      const filepath = path.resolve(dirPath, filename);
      const basename = path.basename(filepath, '.json');
      if (!ignoreTestSchemas.includes(basename)) {
        output.set(basename, readJsonFile(filepath));
      }
    });
    cache.set(dirPath, output);
  }
  return cache.get(dirPath);
};
export const mapToTable = (input: Map<string, JsonSchemaNode>): any[] => {
  const out: any[] = [];
  input.forEach((value, key) => {
    out.push([key, value]);
  });
  return out;
};
export const getSeresJsonSchemasForTest = (): any[] => {
  const dirPath = path.resolve(defaultPath, 'Seres');
  return mapToTable(getJsonSchemasForTest(dirPath));
};

export const getGeneralJsonSchemasForTest = (): any[] => {
  const dirPath = path.resolve(defaultPath, 'General');
  return mapToTable(getJsonSchemasForTest(dirPath));
};

export const getTempJsonSchemasForTest = (): any[] => {
  const dirPath = path.resolve(__dirname, 'dist');
  return mapToTable(getJsonSchemasForTest(dirPath));
};
export const getGeneralJsonSchemaForTest = (name: string): JsonSchemaNode => {
  const dirPath = path.resolve(defaultPath, 'General');
  const schemas = getJsonSchemasForTest(dirPath);
  return schemas.get(name) as JsonSchemaNode;
};

export const dumpToDebug = (dirname: string, basename: string, data: any) => {
  if (process.env.DEBUG === 'true') {
    const dir = path.join(dirname, 'debug');
    if (!fs.existsSync(dir)) {
      fs.mkdirSync(dir);
    }
    const preppedData = data instanceof Map ? Array.from(data) : data;
    const filename = path.join(dir, basename + '.json');
    fs.writeFileSync(filename, JSON.stringify(preppedData, null, 4), 'utf-8');
  }
};

export const validateSchema = (schema: JsonSchemaNode) => {
  return new Ajv2020().validateSchema(schema);
};

export const simpleTestJsonSchema = {
  [Keywords.Properties]: {
    hello: {
      [Keywords.Type]: FieldType.String,
    },
    world: {
      [Keywords.Properties]: {
        hola: {
          [Keywords.Type]: FieldType.Boolean,
        },
      },
    },
  },
};

export const getOldJsonSchemaForTest = () => {
  const filepath = path.resolve(__dirname, 'old-schema.json');
  return readJsonFile(filepath);
};
